<?php
require('../vendor/autoload.php');

use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

if ($pdo == null) {
    exit('Connection error!');
}

$onyen = $_POST["onyen"];

// doing it this way allows multiple games to be inserted into this data array
// this can be refactored either to use a function in the future or to use an alternate method
$data = array();

$stmt = $pdo->prepare('SELECT Score, TimesPlayed FROM dbo.PasswordPlatformerScores WHERE Onyen = :onyen;');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();

$row = $stmt->fetchAll(PDO::FETCH_ASSOC);
$count = !$row ? 0 : 1;

if ($count == 0) {
    $data['Password Platformer'] = array('timesPlayed' => 0);
} else {
    $data['Password Platformer'] = array('timesPlayed' => $row[0]['TimesPlayed'], 'score' => $row[0]['Score']);
}

$stmt = $pdo->prepare('SELECT Score, AttacksCaught FROM dbo.MessageBoardScores WHERE Onyen = :onyen;');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();

$row = $stmt->fetchAll(PDO::FETCH_ASSOC);
$count = !$row ? 0 : 1;

if ($count == 0) {
    $data['Message Board'] = array('timesPlayed' => 0, 'score' => 0);
} else {
    if ($row[0]['AttacksCaught'] == null) {
        $data['Message Board'] = array('timesPlayed' => 0, 'score' => $row[0]['Score']);
    } else {
        $data['Message Board'] = array('timesPlayed' => $row[0]['AttacksCaught'], 'score' => $row[0]['Score']);
    }
}

header('Content-Type: application/json');
print json_encode($data);
