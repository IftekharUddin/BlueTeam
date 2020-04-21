<?php
require('../vendor/autoload.php');

use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

if ($pdo == null) {
    exit('Connection error!');
}

// have to join this wih message board data when available
$query = 'SELECT dbo.PasswordPlatformerScores.Onyen AS "Onyen", 
            dbo.PasswordPlatformerScores.Score AS "Password Platformer", 
            dbo.MessageBoardScores.Score AS "Message Board", 
            ISNULL(dbo.PasswordPlatformerScores.Score, 0) + ISNULL(dbo.MessageBoardScores.Score, 0) AS "Total" 
            FROM dbo.PasswordPlatformerScores 
            FULL OUTER JOIN dbo.MessageBoardScores 
            ON dbo.PasswordPlatformerScores.Onyen = dbo.MessageBoardScores.Onyen 
            ORDER BY Total DESC;';
$stmt = $pdo->prepare($query);
$stmt->execute();

$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

header('Content-Type: json/application;');
echo json_encode($results);
