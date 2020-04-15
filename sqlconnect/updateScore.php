<?php

	ini_set('display_errors', 1); error_reporting(-1);

	// echo phpinfo();

        // Configuration

	$serverName = "FO-COMPSEWEB"; //serverName\instanceName, portNumber (default is 1433)
        $connectionInfo = array( "Database"=>"BlueTeamTables", "UID"=>"LocalDaemonCompSecWebBubba", "PWD" => "TheTimeHasComeTheWalrusSaid08Apr2020*" );
        $conn = sqlsrv_connect( $serverName, $connectionInfo );
 
        if( $conn ) {
                echo "Connection established.<br />";
        }else{
                echo "Connection could not be established.<br />";
                die( print_r( sqlsrv_errors(), true));
        }

	$onyen = $_POST["onyen"];
	$score = $_POST["score"];
            
        $sql = "SELECT *  
                FROM dbo.PasswordPlatformerScores
                WHERE Onyen = 'tas127';";



        $stmt = sqlsrv_query($conn, $sql);
        if( $stmt === false ) {
                die( print_r( sqlsrv_errors(), true));
        }

        // Make the first (and in this case, only) row of the result set available for reading.
        if( sqlsrv_fetch( $stmt ) === false) {
                die(print_r( sqlsrv_errors(), true));
        }

        $row = sqlsrv_get_field($stmt, 0);

        if($row == null){
                $sql = "INSERT INTO PasswordPlatformerScores (Onyen, Score) VALUES (?, ?);";
                $var = array($onyen, $score);
                $stmt = sqlsrv_query($conn, $sql, $var);
        } else {
                $sql = "UPDATE PasswordPlatformerScores
                        SET Score = '" . $score . "' 
                        WHERE Onyen = '" . $onyen . "'
                        AND Score < '" . $score . "';";
                $stmt = sqlsrv_query($conn, $sql);
        }

        if( $stmt === false ) {
                die( print_r( sqlsrv_errors(), true));
        }

?>
