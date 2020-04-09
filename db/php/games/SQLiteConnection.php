<?php
namespace Games;

/**
 * SQLite connection
 */
class SQLiteConnection {
	/**
	 * PDO instance
	 * @var type
	 */
	private $pdo;

	/**
	 * return an instance of PDO object that connects to the SQLite database
	 * @return \PDO
	 */
	public function connect() {
		if($this->pdo == null) {
			try {
				$this->pdo = new \PDO("sqlite:" . Config::PATH_TO_SQLITE_FILE);
			
			} catch (\PDOException $e) {
				//handle exception...
				echo $e;
			}
		}
		return $this->pdo;
	}
}
