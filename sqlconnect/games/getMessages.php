<?php

// Created off the foundation of Used with help from this website https://www.w3schools.com/php/php_ajax_database.asp

require('../vendor/autoload.php');

// connect
use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

// if connection fails
if ($pdo == null) {
    exit('Connection error!');
}

// query 
$stmt = $pdo->prepare('SELECT * FROM dbo.MessageBoardMessages;');
$stmt->execute();

// get query results 
$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

// send query results to js
header('Content-Type: json/application;');
echo json_encode($results);
