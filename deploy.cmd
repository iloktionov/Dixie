del /F /S /Q Deploy\* 
xcopy Dixie.Presentation\bin\*.exe Deploy\ /Y
xcopy Dixie.Presentation\bin\*.dll Deploy\ /Y
xcopy Dixie.Presentation\bin\*.pdb Deploy\ /Y
xcopy Dixie.Presentation\bin\Dixie.Presentation.exe.config Deploy\ /Y
xcopy Dixie.Core\bin\*.example Deploy\ /Y
xcopy Dixie.Console\bin\Dixie.Console* Deploy\ /Y
xcopy Dixie.Algorithms\bin\Dixie.Algorithms.dll Deploy\Algorithms\ /Y
xcopy Dixie.Algorithms\bin\Dixie.Algorithms.pdb Deploy\Algorithms\ /Y
ren Deploy\dixie.Topology.example dixie.Topology
ren Deploy\dixie.Engine.example dixie.Engine

