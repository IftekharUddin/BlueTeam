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

$stmt = $pdo->prepare('SELECT TimesPlayed FROM dbo.PasswordPlatformerScores WHERE Onyen = :onyen;');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();

$row = $stmt->fetchAll(PDO::FETCH_ASSOC);
$count = !$row ? 0 : 1;

if ($count == 0) {
    $data['Password Platformer'] = 0;
} else {
    $data['Password Platformer'] = $row[0]['TimesPlayed'];
}


header('Content-Type: application/json');
print json_encode($data);
