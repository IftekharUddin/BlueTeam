
<?php
require('../vendor/autoload.php');

use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

if ($pdo == null) {
    exit('Connection error!');
}

$stmt = $pdo->prepare(' SELECT Onyen, Score
                        FROM MessageBoardMessages 
                        ORDER BY Score DESC');
$stmt->execute();

$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

header('Content-Type: json/application;');
if (!$results == 0) {
    // nicely handle empty result by giving back empty array 
    echo json_encode(array());
    exit(0);
}

echo json_encode($results);
exit(0);
