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
$score = $_POST["score"];

// query to check if user already exists in table
$stmt = $pdo->prepare('SELECT * FROM dbo.PasswordPlatformerScores WHERE Onyen = :onyen;');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();

// get query data
$row = $stmt->fetchAll(PDO::FETCH_ASSOC);
$count = !$row ? 0 : 1;


// if user doesn't exist yet, insert for the first time
if ($count == 0) {
    $stmt = $pdo->prepare('INSERT INTO dbo.PasswordPlatformerScores (Onyen, Score, TimesPlayed) VALUES (:onyen, :score, 1);');
    $stmt->bindParam(':onyen', $onyen);
    $stmt->bindParam(':score', $score, PDO::PARAM_INT);
    $stmt->execute();

    // updating for the first time affects total leaderboard
    include 'updateLeaderboard.php';
} else {
    $oldScore = $row[0]['Score'];

    if ($score > $oldScore) {
        $stmt = $pdo->prepare('UPDATE dbo.PasswordPlatformerScores SET Score = :score WHERE Onyen = :onyen;');
        $stmt->bindParam(':score', $score, PDO::PARAM_INT);
        $stmt->bindParam(':onyen', $onyen);
        $stmt->execute();

        // update leaderboard since your new high score might affect placements on overall leaderboard
        include 'updateLeaderboard.php';
    }

    // regardless of the new score, update timesPlayed
    $stmt = $pdo->prepare('UPDATE dbo.PasswordPlatformerScores SET TimesPlayed = TimesPlayed + 1 WHERE Onyen = :onyen;');
    $stmt->bindParam(':onyen', $onyen);
    $stmt->execute();
}
exit(0);
