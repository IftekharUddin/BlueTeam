# INTRO

to be proficient in this backend side,

* get used to debugging SQL on the server. that info should be on the game design doc
* get used to php
* the better you are at JS, the more cohension you'll have with your front end lead.
  * but I'm garbage at JS. And yet, things got done.

If I did my job right, nobody should be "dedicated" on the backend.
This should be simple additions here and there unless you want to take on the challenge of optimizing our existing work.

good luck :)

## SQL Connect code

------------------------------------

```php
require('../vendor/autoload.php');

use Games\SQLConnect;

$pdo = (new SQLConnect())->connect();

if ($pdo == null) {
    exit('Connection error!');
}
```

------------------------------------
The code above is required on all php files with the exception of updateLeaderboard

## Vendor

Don't touch the vendor folder unless you're adding more php packages.
Composer: [link](https://getcomposer.org/doc/01-basic-usage.md)

## SQLConnect.php

SQL Connect is the base code that allows us to repeat the connection code multiple times.
Nothing about that file should be adjusted. If you need to change server info, that'll be in the config file.

## Config file

It's on the server because it has server user/pass.
Not something we want to upload to github.

## General Query creation

------------------------------------

```php
$stmt = $pdo->prepare('SELECT * FROM dbo.MessageBoardMessages;');
$stmt->execute();
```

------------------------------------
This is the most basic query. Create a statement. Prepare the SQL Code
Execute

------------------------------------

```php
$stmt = $pdo->prepare('INSERT INTO dbo.PasswordPlatformerScores (Onyen, Score, TimesPlayed) VALUES (:onyen, :score, 1);');
$stmt->bindParam(':onyen', $onyen);
$stmt->bindParam(':score', $score, PDO::PARAM_INT);
$stmt->execute();
```

------------------------------------
If you want parameters, follow this next pattern.

## $_Post

------------------------------------

```php
$onyen = $_POST["onyen"];
$score = $_POST["score"];
```

------------------------------------
When you create a Unity web request or an AJAX request, you have things to send through the post parameters.
get the value associated with the key onyen and store it into $onyen.

you'll only get errors here if you mistype the key

## "get" .phps

------------------------------------

```php
$results = $stmt->fetchAll(PDO::FETCH_ASSOC);

header('Content-Type: json/application;');
echo json_encode($results);
```

------------------------------------

this is how you send data from the sql request to the javascript file.
[Link](https://www.php.net/manual/en/pdostatement.fetch.php)

## include 'updateLeaderboard.php'

include is simple
[Link 1](https://www.tutorialrepublic.com/php-tutorial/php-include-files.php)
[Link 2](https://www.php.net/manual/en/function.include.php)

if you see this line, php treats it as if you've copy pasted updateLeaderboard.php right into that line.
It's a good way to insert repeatable long lines of code.
It could be done as a class like sqlconnect.php but this seemed simpler to me.

we'll talk about the code in chunks

1. We're creating empty variables of types and setting the onyen variable to the post variable onyen. The only reason that the :onyen can work is because we're "include"ing it into other files. The scope of the $_POST in those files like updatePasswordPlatformer.php allows for $onyen to be referenced. See [here](https://docs.microsoft.com/en-us/sql/t-sql/language-elements/variables-transact-sql?view=sql-server-ver15).
2. Delete user from the overall leaderboard. Not doing this causes duplication. This doesn't affect order because when we getLeaderboard, we select by descending order
3. Select score from MessageBoardScores. Insert into variable. Check if it's null (user hasn't played that yet), if so set the variable to zero. [isnull link](https://stackoverflow.com/questions/27813871/isnull-returning-null)
4. Select score from PasswordPlatformer with same process as chunk 3
5. Add up all the scores, insert that new cumulative score into the overall leaderboard

## When adding a game

1. Add a new line in the JS to map from the name of the game to the name of the table.
2. Create an updateScore function/file that should be almost identical to the existing.
3. UPDATE updateLeaderboard.php
    * add a variable in chunk 1 to hold that value
    * add a chunk after chunk 4 almost identical to it to get the score and check null
    * add the value to the equation in the final chunk.
