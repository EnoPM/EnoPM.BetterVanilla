#pragma once
#include "entry-point.h"

inline void init_console()
{
    AllocConsole();
    (void)freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
    // solve the disorder codes in the CJK character show
    SetConsoleOutputCP(CP_UTF8);
}
