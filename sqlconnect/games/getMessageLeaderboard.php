
<?php
require('../vendor/autoload.php');

// connect
use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

// if connection fails
if ($pdo == null) {
    exit('Connection error!');
}

// query for message board scores
$stmt = $pdo->prepare(' SELECT Onyen, Score
                        FROM MessageBoardMessages 
                        ORDER BY Score DESC');
$stmt->execute();

// get query data
$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

// send query data to js
header('Content-Type: json/application;');
echo json_encode($results);
exit(0);