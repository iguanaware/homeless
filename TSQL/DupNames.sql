-- Clean up duplicate names and long names (Not allowed in MDM)
-- varchar limited; NVARCHAR has more superscripts
	
DROP TABLE IF EXISTs #TEXT;
SELECT 1 Idx,CAST('' AS VARCHAR(10)) appendtext INTO #TEXT
UNION SELECT 2,'¹'
UNION SELECT 3,'²'
UNION SELECT 4,'³'
UNION SELECT 5,';4'
UNION SELECT 6,';5'
UNION SELECT 7,';6'
UNION SELECT 8,';7'
UNION SELECT 9,';8'
UNION SELECT 10,';9'

;
WITH FMT AS (
SELECT  [DIAGNOSIS_CD_WITH_SEPARATOR]
, ROW_NUMBER() OVER (PARTITION BY A.DIAGNOSIS_NAME ORDER BY [DIAGNOSIS_CD_WITH_SEPARATOR]) Idx
        FROM   [staging].[icd10_import] A
               JOIN (SELECT DIAGNOSIS_NAME,
                            Count(*) cnt
                     FROM   [staging].[icd10_import]
                     GROUP  BY DIAGNOSIS_NAME
                     HAVING Count(*) > 1) Z
                 ON A.DIAGNOSIS_NAME = Z.DIAGNOSIS_NAME
				 )
 
UPDATE T
SET DIAGNOSIS_NAME=RTRIM(DIAGNOSIS_NAME)+#TEXT.appendtext
FROM  [staging].[icd10_import] T
JOIN FMT ON FMT.[DIAGNOSIS_CD_WITH_SEPARATOR] = T.[DIAGNOSIS_CD_WITH_SEPARATOR]
JOIN #TEXT ON #TEXT.Idx = FMT.Idx
;

