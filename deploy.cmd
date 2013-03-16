del /F /S /Q Deploy\* 
xcopy Dixie.Console\bin\* Deploy\ /Y
xcopy Dixie.Algorithms\bin\Dixie.Algorithms.dll Deploy\Algorithms\ /Y
xcopy Dixie.Algorithms\bin\Dixie.Algorithms.pdb Deploy\Algorithms\ /Y

