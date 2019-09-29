#pragma once
#include <map>
#include "Utils/FilesystemUtils.h"
#include "Utils/VectorUtils.h"
#include "Asset.h"
#include <memory>

namespace AssetManagement {
	class AssetManager {
	public:
		AssetManager(Path inAssetDir);

	private:
		static Vector<Path> FindAllAssetPaths(Path dir);
		void InitAssets();
		void AddAsset(std::unique_ptr<Asset> asset);

		Path mainAssetDir;
		std::map<uuids::uuid, std::unique_ptr<Asset>> assets;
	};
}