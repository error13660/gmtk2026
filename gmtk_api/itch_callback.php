<?php

declare(strict_types=1);

header('Content-Type: application/json; charset=utf-8');
header('Cache-Control: no-store, no-cache, must-revalidate');

/*
 * A callback.html és az itch_callback.php ugyanazon
 * a domainen és protokollon fut, ezért itt nincs
 * szükség CORS fejlécre.
 */

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    sendJson(
        405,
        false,
        'Csak POST kérés engedélyezett.'
    );
}

try {
    $data = readJsonBody();

    $accessToken = isset($data['access_token'])
        ? trim((string) $data['access_token'])
        : '';

    $state = isset($data['state'])
        ? trim((string) $data['state'])
        : '';

    if ($accessToken === '') {
        sendJson(
            400,
            false,
            'Hiányzik az itch.io access token.'
        );
    }

    /*
     * A Unity Guid.NewGuid().ToString("N") hívása
     * 32 darab hexadecimális karaktert generál.
     */
    if (!isValidState($state)) {
        sendJson(
            400,
            false,
            'Érvénytelen OAuth state azonosító.'
        );
    }

    $profile = getItchProfile($accessToken);

    if (
        !isset($profile['user']) ||
        !is_array($profile['user'])
    ) {
        sendJson(
            502,
            false,
            'Az itch.io nem adott vissza user adatot.'
        );
    }

    $user = $profile['user'];

    $itchUserId = isset($user['id'])
        ? filter_var(
            $user['id'],
            FILTER_VALIDATE_INT
        )
        : false;

    $username = isset($user['username'])
        ? trim((string) $user['username'])
        : '';

    $displayName = isset($user['display_name'])
        ? trim((string) $user['display_name'])
        : '';

    if ($itchUserId === false || $itchUserId <= 0) {
        sendJson(
            502,
            false,
            'Az itch.io nem adott vissza érvényes user ID-t.'
        );
    }

    if ($username === '') {
        sendJson(
            502,
            false,
            'Az itch.io nem adott vissza felhasználónevet.'
        );
    }

    /*
     * Az access tokent nem mentjük el.
     * Csak az ID-t, nevet és létrehozási időt tároljuk.
     */
    $loginResult = [
        'status' => 'completed',
        'id' => (int) $itchUserId,
        'username' => $username,
        'display_name' => $displayName,
        'created_at' => time()
    ];

    saveLoginResult(
        $state,
        $loginResult
    );

    sendJson(
        200,
        true,
        'Sikeres itch.io bejelentkezés.',
        [
            'id' => (int) $itchUserId,
            'username' => $username,
            'display_name' => $displayName
        ]
    );
} catch (Throwable $exception) {
    sendJson(
        500,
        false,
        $exception->getMessage()
    );
}

function readJsonBody(): array
{
    $json = file_get_contents('php://input');

    if ($json === false || trim($json) === '') {
        sendJson(
            400,
            false,
            'Hiányzik a kérés törzse.'
        );
    }

    $data = json_decode(
        $json,
        true
    );

    if (!is_array($data)) {
        sendJson(
            400,
            false,
            'Hibás JSON formátum.'
        );
    }

    return $data;
}

function isValidState(string $state): bool
{
    return preg_match(
        '/^[a-f0-9]{32}$/',
        $state
    ) === 1;
}

function getItchProfile(string $accessToken): array
{
    $curl = curl_init(
        'https://api.itch.io/profile'
    );

    if ($curl === false) {
        throw new RuntimeException(
            'Nem sikerült elindítani a cURL-t.'
        );
    }

    curl_setopt_array($curl, [
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_HTTPGET => true,
        CURLOPT_TIMEOUT => 15,
        CURLOPT_CONNECTTIMEOUT => 10,

        CURLOPT_HTTPHEADER => [
            'Accept: application/json',
            'Authorization: Bearer ' . $accessToken
        ]
    ]);

    $responseBody = curl_exec($curl);

    if ($responseBody === false) {
        $error = curl_error($curl);

        curl_close($curl);

        throw new RuntimeException(
            'itch.io kapcsolódási hiba: ' . $error
        );
    }

    $statusCode = (int) curl_getinfo(
        $curl,
        CURLINFO_HTTP_CODE
    );

    curl_close($curl);

    if ($statusCode !== 200) {
        sendJson(
            401,
            false,
            'Az itch.io token érvénytelen vagy lejárt.'
        );
    }

    $profile = json_decode(
        $responseBody,
        true
    );

    if (!is_array($profile)) {
        throw new RuntimeException(
            'Az itch.io válasza nem érvényes JSON.'
        );
    }

    return $profile;
}

function saveLoginResult(
    string $state,
    array $loginResult
): void {
    /*
     * A mappa teljes elérési útja például:
     *
     * /gmtk_api/login_sessions
     */
    $sessionDirectory =
        __DIR__ . '/login_sessions';

    if (!is_dir($sessionDirectory)) {
        $created = mkdir(
            $sessionDirectory,
            0700,
            true
        );

        if (!$created && !is_dir($sessionDirectory)) {
            throw new RuntimeException(
                'Nem sikerült létrehozni a login_sessions mappát.'
            );
        }
    }

    if (!is_writable($sessionDirectory)) {
        throw new RuntimeException(
            'A login_sessions mappa nem írható.'
        );
    }

    $filePath =
        $sessionDirectory .
        DIRECTORY_SEPARATOR .
        $state .
        '.json';

    $jsonResult = json_encode(
        $loginResult,
        JSON_UNESCAPED_UNICODE |
        JSON_UNESCAPED_SLASHES
    );

    if ($jsonResult === false) {
        throw new RuntimeException(
            'Nem sikerült elkészíteni a login eredmény JSON-t.'
        );
    }

    $writtenBytes = file_put_contents(
        $filePath,
        $jsonResult,
        LOCK_EX
    );

    if ($writtenBytes === false) {
        throw new RuntimeException(
            'Nem sikerült elmenteni a login eredményt.'
        );
    }

    /*
     * Csak a webszerver felhasználója férjen hozzá.
     */
    @chmod(
        $filePath,
        0600
    );
}

function sendJson(
    int $statusCode,
    bool $success,
    string $message,
    ?array $data = null
): never {
    http_response_code($statusCode);

    $response = [
        'success' => $success,
        'message' => $message
    ];

    if ($data !== null) {
        $response['data'] = $data;
    }

    echo json_encode(
        $response,
        JSON_UNESCAPED_UNICODE |
        JSON_UNESCAPED_SLASHES |
        JSON_PRETTY_PRINT
    );

    exit;
}