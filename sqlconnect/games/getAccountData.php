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

// array holds each game's data
$data = array();

// query for passwordplatformer data
$stmt = $pdo->prepare('SELECT Score, TimesPlayed FROM dbo.PasswordPlatformerScores WHERE Onyen = :onyen;');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();

// get data from query
$row = $stmt->fetchAll(PDO::FETCH_ASSOC);
$count = !$row ? 0 : 1;

/* 
    check if user has no data for password platformer 
    if so, timesplayed = 0
    otherwise, store as standard
*/
if ($count == 0) {
    $data['Password Platformer'] = array('timesPlayed' => 0);
} else {
    $data['Password Platformer'] = array('timesPlayed' => $row[0]['TimesPlayed'], 'score' => $row[0]['Score']);
}

// query for message board data 
$stmt = $pdo->prepare('SELECT Score, AttacksCaught FROM dbo.MessageBoardScores WHERE Onyen = :onyen;');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();

// get message board data
$row = $stmt->fetchAll(PDO::FETCH_ASSOC);
$count = !$row ? 0 : 1;

/* 
    check if user has no data for Message Board 
    if so, timesplayed = 0, score = 0
    otherwise, store as standard
*/
if ($count == 0) {
    $data['Message Board'] = array('timesPlayed' => 0, 'score' => 0);
} else {
    if ($row[0]['AttacksCaught'] == null) {
        $data['Message Board'] = array('timesPlayed' => 0, 'score' => $row[0]['Score']);
    } else {
        $data['Message Board'] = array('timesPlayed' => $row[0]['AttacksCaught'], 'score' => $row[0]['Score']);
    }
}

// send data to js
header('Content-Type: application/json');
print json_encode($data);
exit(0);
