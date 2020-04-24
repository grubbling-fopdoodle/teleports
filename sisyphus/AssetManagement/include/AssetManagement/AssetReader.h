#pragma once
#include <string>
#include <memory>
#include "AssetManagement/Asset.h"

// class AssetIterator;
namespace Sisyphus::AssetManagement {
	class AssetReader {
	public:
		enum class ReaderType {
			Unpacked,
			Packed
		};

		static std::unique_ptr<AssetReader> Create(ReaderType type);

		virtual ~AssetReader() = default;

		virtual void ReadAssets(std::string directory) = 0;
		virtual const Asset& GetAsset(uuids::uuid id) const = 0;
		virtual int AssetCount() const = 0;
		// TODO: virtual AssetIterator GetIterator() = 0;
	};

}