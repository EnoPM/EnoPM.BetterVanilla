#pragma once
#include "version-proxy/version.h"
#include <filesystem>

std::filesystem::path get_application_path();

DWORD WINAPI main_entry_point(LPVOID lp_param);
