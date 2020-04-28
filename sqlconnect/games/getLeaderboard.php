<?php
require('../vendor/autoload.php');

// connect
use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

// if connection fails 
if ($pdo == null) {
    exit('Connection error!');
}

// query to select top 10 
$stmt = $pdo->prepare(' SELECT TOP 10 Onyen, Total 
                        FROM Overall_Leaderboard
                        ORDER BY Total DESC;');
$stmt->execute();

// get query data
$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

// send query data to js
header('Content-Type: json/application;');
echo json_encode($results);
exit(0);
