<?php

namespace Games;

/**
 * MSSQL connection
 */
class SQLConnect
{
    /**
     * PDO instance
     * @var type
     */
    private $pdo;

    /**
     * return an instance of PDO object that connects to the SQL Server database
     * @return PDO
     */
    public function connect()
    {
        if ($this->pdo == null) {
            try {
                $this->pdo = new \PDO("sqlsrv:server=" . Config::SERVER . ";Database = " . Config::DB, Config::USER, Config::PASSWORD);
                $this->pdo->setAttribute(\PDO::ATTR_ERRMODE, \PDO::ERRMODE_EXCEPTION);
            } catch (\PDOException $e) {
                //handle exception...
                echo $e;
                die("Error connecting to SQL Server ...");
            }
        }
        return $this->pdo;
    }
}
