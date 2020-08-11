#pragma once
#include <vector>
#include <common/hashmap.h>
#include <common/hash-str.h>

using namespace Common;

typedef HashMap<Common::String, std::vector<byte>, Common::IgnoreCase_Hash, Common::IgnoreCase_EqualTo> SaveFileCache;
