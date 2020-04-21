<?php
require('../vendor/autoload.php');

use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

if ($pdo == null) {
    exit('Connection error!');
}

// have to join this wih message board data when available
$stmt = $pdo->prepare('SELECT Onyen, Score FROM dbo.PasswordPlatformerScores ORDER BY Score DESC;');
$stmt->execute();

$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

header('Content-Type: json/application;');
echo json_encode($results);
