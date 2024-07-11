#include <windows.h>

#include "entry-point.h"
#include "version-proxy/version.h"

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        DisableThreadLibraryCalls(hModule);
        CreateThread(nullptr, 0, main_entry_point, hModule, NULL, nullptr);
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        FreeLibrary(version_dll);
        break;
    case DLL_PROCESS_DETACH:
    default:
        break;
    }

    return TRUE;
}


