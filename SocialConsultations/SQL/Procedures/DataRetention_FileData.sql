CREATE OR ALTER PROCEDURE DataRetention_FileData
(
	@Hours INT
)
AS
BEGIN
	DELETE FROM FileData
	WHERE CreateDate IS NOT NULL 
	AND DATEDIFF(hour, CreateDate, GETUTCDATE()) >= @Hours
	AND SolutionId IS NULL AND IssueId IS NULL
	AND Id NOT IN (
		SELECT DISTINCT AvatarId as 'Id'
		FROM Users WHERE AvatarId IS NOT NULL)
END
GO