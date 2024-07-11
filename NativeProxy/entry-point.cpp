#include "entry-point.h"

#include "console.h"

std::filesystem::path get_application_path()
{
    TCHAR buff[MAX_PATH];
    GetModuleFileName(nullptr, buff, MAX_PATH);
    return {buff};
}

DWORD WINAPI main_entry_point(LPVOID lp_param)
{
    if (!check_entry_point())
    {
        return 0;
    }

    if (get_application_path().filename() != "Among Us.exe") return 0;

    init_console();

    return 0;
}
