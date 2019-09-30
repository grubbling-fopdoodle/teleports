#pragma once
#include "uuid.h"
#include "json.hpp"
#include "Utils\StringUtils.h"
#include "Utils\UuidJsonSerializer.h"

namespace AssetManagement {
	struct AssetMetadata {
		uuids::uuid id;
		String name;
		bool isBinary;
	};
}

namespace nlohmann {
	template<>
	struct adl_serializer<AssetManagement::AssetMetadata> {
		static void to_json(json& j, const AssetManagement::AssetMetadata& metadata) {
			j["id"] = metadata.id;
			j["name"] = metadata.name;
			j["isBinary"] = metadata.isBinary;
		}
		static void from_json(const json& j, AssetManagement::AssetMetadata& metadata) {
			metadata.id = j["id"].get<uuids::uuid>();
			metadata.name = j["name"];
			metadata.isBinary = j["isBinary"].get<bool>();
		}
	};
}