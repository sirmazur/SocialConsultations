CREATE OR ALTER PROCEDURE DataRetention_FileData
(
	@Hours INT
)
AS
BEGIN
	UPDATE FileData
	SET DeletionMarkTimestamp = GETUTCDATE()
	WHERE DeletionMarkTimestamp IS NULL 
	AND SolutionId IS NULL AND IssueId IS NULL
	AND Id NOT IN (
		SELECT DISTINCT AvatarId as 'Id'
		FROM Users WHERE AvatarId IS NOT NULL
		UNION
		SELECT DISTINCT AvatarId as 'Id'
		FROM Communities WHERE AvatarId IS NOT NULL
		UNION
		SELECT DISTINCT BackgroundId as 'Id'
		FROM Communities WHERE BackgroundId IS NOT NULL)

	DELETE FROM FileData
	WHERE DeletionMarkTimestamp IS NOT NULL 
	AND DATEDIFF(hour, DeletionMarkTimestamp, GETUTCDATE()) >= @Hours
	AND SolutionId IS NULL AND IssueId IS NULL
	AND Id NOT IN (
		SELECT DISTINCT AvatarId as 'Id'
		FROM Users WHERE AvatarId IS NOT NULL
		UNION
		SELECT DISTINCT AvatarId as 'Id'
		FROM Communities WHERE AvatarId IS NOT NULL
		UNION
		SELECT DISTINCT BackgroundId as 'Id'
		FROM Communities WHERE BackgroundId IS NOT NULL)
END
GO