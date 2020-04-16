<?php

// Created off the foundation of Used with help from this website https://www.w3schools.com/php/php_ajax_database.asp

require('../vendor/autoload.php');

use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

if ($pdo == null) {
    exit('Connection error!');
}

$stmt = $pdo->prepare('SELECT * FROM dbo.MessageBoardMessages;');
$stmt->execute();

$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

header('Content-Type: json/application;');
echo json_encode($results);
