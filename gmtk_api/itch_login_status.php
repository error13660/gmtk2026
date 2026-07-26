<?php

declare(strict_types=1);

header('Content-Type: application/json; charset=utf-8');
header('Cache-Control: no-store');

header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(204);
    exit;
}

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    sendJson(
        405,
        false,
        'Csak GET kérés engedélyezett.'
    );
}

$state = isset($_GET['state'])
    ? trim((string) $_GET['state'])
    : '';

if (!preg_match('/^[a-f0-9]{32}$/', $state)) {
    sendJson(
        400,
        false,
        'Érvénytelen state.'
    );
}

$sessionDirectory = __DIR__ . '/login_sessions';
$filePath = $sessionDirectory . '/' . $state . '.json';

if (!is_file($filePath)) {
    sendJson(
        200,
        true,
        'A bejelentkezés még folyamatban van.',
        [
            'status' => 'pending'
        ]
    );
}

$fileContents = file_get_contents($filePath);

if ($fileContents === false) {
    sendJson(
        500,
        false,
        'Nem sikerült beolvasni a login eredményt.'
    );
}

$data = json_decode($fileContents, true);

if (!is_array($data)) {
    @unlink($filePath);

    sendJson(
        500,
        false,
        'A login eredmény sérült.'
    );
}

$createdAt = isset($data['created_at'])
    ? (int) $data['created_at']
    : 0;

if ($createdAt === 0 || time() - $createdAt > 600) {
    @unlink($filePath);

    sendJson(
        410,
        false,
        'A login eredmény lejárt.'
    );
}

/*
 * Egyszer használatos eredmény:
 * kiolvasás után töröljük.
 */
@unlink($filePath);

sendJson(
    200,
    true,
    'Sikeres itch.io bejelentkezés.',
    [
        'status' => 'completed',
        'id' => (int) $data['id'],
        'username' => (string) $data['username'],
        'display_name' =>
            (string) ($data['display_name'] ?? '')
    ]
);

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