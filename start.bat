start "NCC1" .\NCCServer\NCCServer\bin\Debug\ControlPlaneServer.exe "conf\NCC1Config.xml"
start "NCC2" .\NCCServer\NCCServer\bin\Debug\ControlPlaneServer.exe "conf\NCC2Config.xml"
start "CPCC1" .\CPCC\KolejneCPCC\bin\Debug\KolejneCPCC.exe "conf\CPCC01Config.xml"
start "CPCC2" .\CPCC\KolejneCPCC\bin\Debug\KolejneCPCC.exe "conf\CPCC02Config.xml"
start "CPCC3" .\CPCC\KolejneCPCC\bin\Debug\KolejneCPCC.exe "conf\CPCC03Config.xml"
start "D1" .\Subnetwork\Subnetwork\bin\debug\Subnetwork.exe "conf\domena1_config.xml" "conf\PD1Config.xml" "conf\P1Config.xml" "conf\P2Config.xml"
start "D2" .\Subnetwork\Subnetwork\bin\debug\Subnetwork.exe "conf\domena2_config.xml" "conf\PD2Config.xml" "conf\P3Config.xml" "conf\P4Config.xml"
start "MC1" .\ManagementCenter\ManagementCenter\bin\Debug\ManagementCenter.exe "conf\MC1Config.xml"
start "MC2" .\ManagementCenter\ManagementCenter\bin\Debug\ManagementCenter.exe "conf\MC2Config.xml"
start "CC" .\CableCloud\CableCloud\bin\Debug\CableCloud.exe "conf\CableCloudConfig.xml"