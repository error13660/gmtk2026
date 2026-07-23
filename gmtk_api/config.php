<?php

declare(strict_types=1);

header('Content-Type: application/json; charset=utf-8');

function loadEnv($filePath)
{
    if (!file_exists($filePath)) {
        throw new RuntimeException('A .env fájl nem található.');
    }

    $lines = file($filePath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);

    foreach ($lines as $line) {
        $line = trim($line);

        if ($line === '' || strpos($line, '#') === 0) {
            continue;
        }

        $parts = explode('=', $line, 2);

        if (count($parts) !== 2) {
            continue;
        }

        $key = trim($parts[0]);
        $value = trim($parts[1]);

        if (
            strlen($value) >= 2 &&
            (
                ($value[0] === '"' && substr($value, -1) === '"') ||
                ($value[0] === "'" && substr($value, -1) === "'")
            )
        ) {
            $value = substr($value, 1, -1);
        }

        $_ENV[$key] = $value;
        putenv($key . '=' . $value);
    }
}

try {
    loadEnv(__DIR__ . '/.env');

    $host = isset($_ENV['DB_HOST'])
        ? $_ENV['DB_HOST']
        : 'localhost';

    $dbname = isset($_ENV['DB_NAME'])
        ? $_ENV['DB_NAME']
        : 'gmtk_2026';

    $username = isset($_ENV['DB_USERNAME'])
        ? $_ENV['DB_USERNAME']
        : '';

    $password = isset($_ENV['DB_PASSWORD'])
        ? $_ENV['DB_PASSWORD']
        : '';

    if ($username === '') {
        throw new RuntimeException(
            'A DB_USERNAME nincs beállítva a .env fájlban.'
        );
    }

    $pdo = new PDO(
        'mysql:host=' . $host .
        ';dbname=' . $dbname .
        ';charset=utf8mb4',
        $username,
        $password,
        [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
            PDO::ATTR_EMULATE_PREPARES => false
        ]
    );
} catch (Exception $exception) {
    http_response_code(500);

    echo json_encode(
        [
            'success' => false,
            'message' => $exception->getMessage()
        ],
        JSON_UNESCAPED_UNICODE
    );

    exit;
}