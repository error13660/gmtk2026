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
} catch (Exception $exception) {
    sendJson(
        500,
        false,
        $exception->getMessage()
    );
}

function handleGet(PDO $pdo): void
{
    $playerName = isset($_GET['player_name'])
        ? trim($_GET['player_name'])
        : null;

    if ($playerName !== null && $playerName !== '') {
        $statement = $pdo->prepare(
            'select id, player_name, score, depth, updated_at
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
        'select id, player_name, score, depth, updated_at
         from leaderboard
         order by score desc, depth desc'
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
    $data = json_decode($json, true);

    if (!is_array($data)) {
        sendJson(
            400,
            false,
            'Hibás JSON formátum.'
        );
    }

    $playerName = isset($data['player_name'])
        ? trim((string) $data['player_name'])
        : '';

    $score = filter_var(
        isset($data['score']) ? $data['score'] : null,
        FILTER_VALIDATE_INT
    );

    $depth = filter_var(
        isset($data['depth']) ? $data['depth'] : null,
        FILTER_VALIDATE_INT
    );

    if ($playerName === '') {
        sendJson(
            400,
            false,
            'A player_name mező kötelező.'
        );
    }

    if ($score === false || $score < 0) {
        sendJson(
            400,
            false,
            'A score mezőnek nem negatív egész számnak kell lennie.'
        );
    }

    if ($depth === false || $depth < 0) {
        sendJson(
            400,
            false,
            'A depth mezőnek nem negatív egész számnak kell lennie.'
        );
    }

    $statement = $pdo->prepare(
        'insert into leaderboard (player_name, score, depth)
         values (:player_name, :score, :depth)
         on duplicate key update
             score = values(score),
             depth = values(depth)'
    );

    $statement->execute([
        'player_name' => $playerName,
        'score' => $score,
        'depth' => $depth
    ]);

    $selectStatement = $pdo->prepare(
        'select id, player_name, score, depth, updated_at
         from leaderboard
         where player_name = :player_name'
    );

    $selectStatement->execute([
        'player_name' => $playerName
    ]);

    $player = $selectStatement->fetch();

    sendJson(
        200,
        true,
        'A játékos adatai sikeresen mentve.',
        $player
    );
}

function sendJson(
    $statusCode,
    $success,
    $message,
    $data = null
) {
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
        JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT
    );

    exit;
}