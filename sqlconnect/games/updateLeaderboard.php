<?php
$stmt = $pdo->prepare('DECLARE @Mscore int,
                                @PScore int,
                                @Total int,
                                @Onyen varchar(50)
                        SET @Onyen = :onyen

                        DELETE FROM Overall_Leaderboard
                        WHERE Onyen = @Onyen

                        SELECT @Mscore = Score
                        FROM MessageBoardScores
                        WHERE Onyen = @Onyen
                        SELECT @Mscore = ISNULL(@Mscore, 0)

                        SELECT @PScore = Score
                        FROM PasswordPlatformerScores
                        WHERE Onyen = @Onyen
                        SELECT @Pscore = ISNULL(@Pscore, 0)

                        SET @Total = @Mscore + @Pscore
                        INSERT INTO Overall_Leaderboard (Onyen, Score)
                        VALUES (@Onyen, @Total)O');
$stmt->bindParam(':onyen', $onyen);
$stmt->execute();