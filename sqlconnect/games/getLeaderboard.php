<?php
require('../vendor/autoload.php');

use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

if ($pdo == null) {
    exit('Connection error!');
}

// have to join this wih message board data when available
$query ='SELECT TOP 10 Onyen, Total 
         FROM Overall_Leaderboard
         ORDER BY Total DESC;';
$stmt = $pdo->prepare($query);
$stmt->execute();

$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

header('Content-Type: json/application;');
echo json_encode($results);
