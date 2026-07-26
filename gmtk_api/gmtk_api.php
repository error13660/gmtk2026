<?php

declare(strict_types=1);

header('Content-Type: application/json; charset=utf-8');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, PUT, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(204);
    exit;
}

require_once __DIR__ . '/config.php';

$method = $_SERVER['REQUEST_METHOD'];

try {
    switch ($method) {
        case 'GET':
            handleGet($pdo);
            break;

        case 'PUT':
            handlePut($pdo);
            break;

        default:
            sendJson(
                405,
                false,
                'Nem támogatott HTTP metódus.'
            );
    }
} catch (PDOException $exception) {
    sendJson(
        500,
        false,
        $exception->getMessage()
    );
} catch (Throwable $exception) {
    sendJson(
        500,
        false,
        $exception->getMessage()
    );
}

function handleGet(PDO $pdo): void
{
    $playerName = isset($_GET['player_name'])
        ? trim((string) $_GET['player_name'])
        : null;

    if ($playerName !== null && $playerName !== '') {
        $statement = $pdo->prepare(
            'select id, player_name, depth, updated_at
             from leaderboard
             where player_name = :player_name'
        );

        $statement->execute([
            'player_name' => $playerName
        ]);

        $player = $statement->fetch();

        if (!$player) {
            sendJson(
                404,
                false,
                'A játékos nem található.'
            );
        }

        sendJson(
            200,
            true,
            'Játékos sikeresen lekérve.',
            $player
        );
    }

    $statement = $pdo->query(
        'select id, player_name, depth, updated_at
         from leaderboard
         order by depth desc'
    );

    $players = $statement->fetchAll();

    sendJson(
        200,
        true,
        'Játékosok sikeresen lekérve.',
        $players
    );
}

function handlePut(PDO $pdo): void
{
    $json = file_get_contents('php://input');
    $data = json_decode($json ?: '', true);

    if (!is_array($data)) {
        sendJson(
            400,
            false,
            'Hibás JSON formátum.'
        );
    }

    $userId = filter_var(
        $data['user_id'] ?? null,
        FILTER_VALIDATE_INT
    );

    $playerName = isset($data['player_name'])
        ? trim((string) $data['player_name'])
        : '';

    $depth = filter_var(
        $data['depth'] ?? null,
        FILTER_VALIDATE_INT
    );

    if ($userId === false || $userId <= 0) {
        sendJson(
            400,
            false,
            'A user_id mezőnek pozitív egész számnak kell lennie.'
        );
    }

    if ($playerName === '') {
        sendJson(
            400,
            false,
            'A player_name mező kötelező.'
        );
    }

    if (mb_strlen($playerName) > 255) {
        sendJson(
            400,
            false,
            'A player_name túl hosszú.'
        );
    }

    if ($depth === false || $depth < 0) {
        sendJson(
            400,
            false,
            'A depth mezőnek nem negatív egész számnak kell lennie.'
        );
    }

    /*
     * A user_id oszlop UNIQUE.
     *
     * Új user_id esetén létrehoz egy rekordot.
     * Meglévő user_id esetén frissíti a nevet, de a depth
     * csak akkor változik, ha az új érték nagyobb.
     */
    $statement = $pdo->prepare(
        'insert into leaderboard (
            user_id,
            player_name,
            depth
         )
         values (
            :user_id,
            :player_name,
            :depth
         )
         on duplicate key update
            player_name = values(player_name),
            depth = greatest(depth, values(depth))'
    );

    $statement->execute([
        'user_id' => $userId,
        'player_name' => $playerName,
        'depth' => $depth
    ]);

    /*
     * A mentett rekordot már user_id alapján kérjük vissza.
     */
    $selectStatement = $pdo->prepare(
        'select
            id,
            user_id,
            player_name,
            depth,
            updated_at
         from leaderboard
         where user_id = :user_id'
    );

    $selectStatement->execute([
        'user_id' => $userId
    ]);

    $player = $selectStatement->fetch();

    if (!$player) {
        sendJson(
            500,
            false,
            'A mentett játékost nem sikerült visszaolvasni.'
        );
    }

    sendJson(
        200,
        true,
        'A játékos legjobb eredménye sikeresen mentve.',
        $player
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