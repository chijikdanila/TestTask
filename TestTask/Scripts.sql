USE test_task;
GO
SELECT SUM(CAST(number AS BIGINT)) AS total_sum
FROM random_strings;
GO
SELECT TOP 1 median_value
FROM (
		SELECT PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY [float]) OVER () AS median_value
		FROM random_strings
) AS Median;