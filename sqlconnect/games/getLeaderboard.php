<?php
require('../vendor/autoload.php');

use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

if ($pdo == null) {
    exit('Connection error!');
}

$query = 'SELECT Onyen, Total 
         FROM Overall_Leaderboard
         ORDER BY Total DESC;';
$stmt = $pdo->prepare($query);
$stmt->execute();

$results = $stmt->fetchAll(PDO::FETCH_ASSOC);


header('Content-Type: json/application;');
if (!$results == 0) {
    // nicely handle empty result by giving back empty array 
    echo json_encode(array());
    exit(0);
}

echo json_encode($results);
exit(0);
