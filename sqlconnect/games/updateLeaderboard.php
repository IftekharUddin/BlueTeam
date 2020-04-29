<?php
// README.md - please read
$stmt = $pdo->prepare('DECLARE @Mscore int,
                                @PScore int,
                                @Total int,
                                @Onyen varchar(50)
                        SET @Onyen = :onyen

                        DELETE FROM dbo.Overall_Leaderboard
                        WHERE Onyen = @Onyen

                        SELECT @Mscore = Score
                        FROM dbo.MessageBoardScores
                        WHERE Onyen = @Onyen
                        SELECT @Mscore = ISNULL(@Mscore, 0)

                        SELECT @PScore = Score
                        FROM dbo.PasswordPlatformerScores
                        WHERE Onyen = @Onyen
                        SELECT @Pscore = ISNULL(@Pscore, 0)

                        SET @Total = @Mscore + @Pscore
                        INSERT INTO dbo.Overall_Leaderboard (Onyen, Total)
                        VALUES (@Onyen, @Total)');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();
