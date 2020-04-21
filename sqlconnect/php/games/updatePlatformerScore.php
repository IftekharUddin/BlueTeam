<?php
require('../vendor/autoload.php');

use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

if ($pdo == null) {
    exit('Connection error!');
}

$onyen = $_POST["onyen"];
$score = $_POST["score"];

$stmt = $pdo->prepare('SELECT * FROM dbo.PasswordPlatformerScores WHERE Onyen = :onyen;');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();

$row = $stmt->fetchAll(PDO::FETCH_ASSOC);
$count = !$row ? 0 : 1;

if ($count == 0) {
    $stmt = $pdo->prepare('INSERT INTO dbo.PasswordPlatformerScores (Onyen, Score, TimesPlayed) VALUES (:onyen, :score, 1);');
    $stmt->bindParam(':onyen', $onyen);
    $stmt->bindParam(':score', $score, PDO::PARAM_INT);
    $stmt->execute();
} else {
    $oldScore = $row[0]['Score'];

    if ($score > $oldScore) {
        $stmt = $pdo->prepare('UPDATE dbo.PasswordPlatformerScores SET Score = :score WHERE Onyen = :onyen;');
        $stmt->bindParam(':score', $score, PDO::PARAM_INT);
        $stmt->bindParam(':onyen', $onyen);
        $stmt->execute();
    }

    $stmt = $pdo->prepare('UPDATE dbo.PasswordPlatformerScores SET TimesPlayed = TimesPlayed + 1 WHERE Onyen = :onyen;');
    $stmt->bindParam(':onyen', $onyen);
    $stmt->execute();
}
exit(0);
