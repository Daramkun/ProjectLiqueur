using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android.App;

// 어셈블리의 일반 정보는 다음 특성 집합을 통해 제어됩니다.
// 어셈블리와 관련된 정보를 수정하려면
// 이 특성 값을 변경하십시오.
[assembly: AssemblyTitle ( "Daramkun.Liqueur.Platforms.Android" )]
[assembly: AssemblyDescription ( "Project Liqueur Platform Framework for Google Android" )]
[assembly: AssemblyConfiguration ( "" )]
[assembly: AssemblyCompany ( "Daramkun's NEST" )]
[assembly: AssemblyProduct ( "Daramkun.Liqueur.Platforms.Android" )]
[assembly: AssemblyCopyright ( "Copyright © 2013 Daramkun, Powered by OpenTK for Android, Xamarin Studio" )]
[assembly: AssemblyTrademark ( "Project Liqueur®" )]
[assembly: AssemblyCulture ( "" )]

// ComVisible을 false로 설정하면 이 어셈블리의 형식이 COM 구성 요소에 
// 표시되지 않습니다. COM에서 이 어셈블리의 형식에 액세스하려면 
// 해당 형식에 대해 ComVisible 특성을 true로 설정하십시오.
[assembly: ComVisible ( false )]

// 어셈블리의 버전 정보는 다음 네 가지 값으로 구성됩니다.
//
//      주 버전
//      부 버전 
//      빌드 번호
//      수정 버전
//
// 모든 값을 지정하거나 아래와 같이 '*'를 사용하여 빌드 번호 및 수정 버전이 자동으로
// 지정되도록 할 수 있습니다.
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion ( "1.0.0.0" )]
[assembly: AssemblyFileVersion ( "1.0.0.0" )]

// Add some common permissions, these can be removed if not needed
[assembly: UsesPermission ( Android.Manifest.Permission.Vibrate )]
[assembly: UsesPermission ( Android.Manifest.Permission.WakeLock )]
