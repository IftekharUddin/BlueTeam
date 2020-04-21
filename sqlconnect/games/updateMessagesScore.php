<?php
require('../vendor/autoload.php');

use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

if ($pdo == null) {
    exit('Connection error!');
}

$onyen = $_POST["onyen"];
//Need to change this true to something else - ???????
$score = $_POST["data"];

$stmt = $pdo->prepare('SELECT * FROM dbo.MessageBoardScores WHERE Onyen = :onyen;');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();

$row = $stmt->fetchAll(PDO::FETCH_ASSOC);
$count = !$row ? 0 : 1;
$playerAttacks = $row[2];
// TO get the number of attacks - THIS WILL NEED TO BE CHANGED EVERYTIME A MESSAGE FROM DAN IS PUT ON THE MESSAGEBOARD
$attacks = 1; 

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
exit(0);
