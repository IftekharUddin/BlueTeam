<?php
require('../vendor/autoload.php');

// connect
use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

// if connection fails 
if ($pdo == null) {
    exit('Connection error!');
}

$stmt = $pdo->prepare(' SELECT Onyen, Total 
                        FROM dbo.Overall_Leaderboard
                        ORDER BY Total DESC;');
$stmt->execute();

// get query data
$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

// send query data to js
header('Content-Type: json/application;');
if (!$results) {
    echo json_encode(array());
    exit(0);
}

echo json_encode($results);
exit(0);
