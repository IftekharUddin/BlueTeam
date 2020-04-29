<?php
require('../vendor/autoload.php');

// connect
use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

// if connection fails
if ($pdo == null) {
    exit('Connection error!');
}

$onyen = $_POST["onyen"];
$score = $_POST["data"];

// query to get user data
$stmt = $pdo->prepare('SELECT * FROM dbo.MessageBoardScores WHERE Onyen = :onyen;');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();

// get query data
$row = $stmt->fetchAll(PDO::FETCH_ASSOC);
$count = !$row ? 0 : 1;
$playerAttacks = $row[2];

// IMPORTANT: TO get the number of attacks - THIS WILL NEED TO BE CHANGED EVERYTIME A MESSAGE FROM DAN IS PUT ON THE MESSAGEBOARD
$attacks = 1;

// if user has no previous data
if ($count == 0) {
    $stmt = $pdo->prepare('INSERT INTO dbo.MessageBoardScores (Onyen, Score, AttacksCaught) VALUES (:onyen, :score, 1);');
    $stmt->bindParam(':onyen', $onyen);
    $stmt->bindParam(':score', $score, PDO::PARAM_INT);
    $stmt->execute();
} else {
    if ($playerAttacks < $attack) {
        $stmt = $pdo->prepare('UPDATE dbo.MessageBoardScores SET Score = Score + :score SET AttacksCaught = AttacksCaught + 1 WHERE Onyen = :onyen;');
        $stmt->bindParam(':score', $score, PDO::PARAM_INT); // should be -100 for wrong button and +100 for correct button presses
        $stmt->bindParam(':onyen', $onyen);
        $stmt->execute();
    }
}

// update leaderboard 
// each time an action is taken, messages score is changed
// changing overall leaderboard score
include 'updateLeaderboard.php';

exit(0);
