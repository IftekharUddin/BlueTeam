
<?php
require('../vendor/autoload.php');

// connect
use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

// if connection fails
if ($pdo == null) {
    exit('Connection error!');
}

$table = $_POST['table'];
$scoreColumn = (array_key_exists('total', $_POST)) ? 'Total' : 'Score';

$query = "SELECT Onyen, " . $scoreColumn . " FROM dbo." . $table .  " ORDER BY " . $scoreColumn . " DESC;";
$stmt = $pdo->prepare($query);
$stmt->execute();

// get query data
$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

// send data to js
header('Content-Type: json/application;');
if (!$results) {
    // nicely handle empty result by giving back empty array 
    echo json_encode(array());
    exit(0);
}

echo json_encode($results);
exit(0);
