#include <exception>
#include <iostream>
#include "AssetManagement/AssetReader.h"
#include "Logger/Logger.h"
#include "Renderer/IDrawable.h"
#include "Renderer/RendererFactory.h"
#include "Utils/DebugMacros.h"
#include "Utils/Platform.h"

using namespace Sisyphus;

struct Vertex {
	float x;
	float y;
	float z;
};

class Vertices : public Rendering::IDrawable {
public:
	Vertices(std::vector<Vertex> inVertices) :
		vertices(std::move(inVertices))
	{
	}

	uint32_t GetVertexCount() const override { return static_cast<uint32_t>(vertices.size()); }
	uuids::uuid GetVertexShaderId() const override { return uuids::uuid::from_string("e1124008-e112-4008-a2f3-cf6233498020").value(); }
	size_t GetVertexBufferSize() const override { return vertices.size() * sizeof(Vertex); }
	const std::byte* GetVertexData() const override { return reinterpret_cast<const std::byte*>(vertices.data()); }

private:
	std::vector<Vertex> vertices;
};

// square center is at 0,0
// x, y - top left corner
// z - depth
Vertices MakeSquare(float x, float y, float z) {
	return Vertices{ {
		{x, y, z},
		{-x, y, z},
		{x, -y, z},

		{x, -y, z},
		{-x, y, z},
		{-x, -y, z}
	} };
}

#ifdef SIS_WINDOWS

#include "WindowCreator/WindowCreator.h"

int main() {
	try {
#ifdef SIS_DEBUG
		auto assetReaderType = AssetManagement::AssetReaderType::Unpacked;
#else 
		auto assetReaderType = AssetManagement::AssetReaderType::Packed;
#endif
		auto assetReader = AssetManagement::AssetReader::Create(assetReaderType);
		assetReader->Read("assets");

		using namespace Rendering;
		namespace wc = WindowCreator;

		wc::WindowCreator windowCreator;
		auto window = windowCreator.Create(wc::WindowCreateInfo{
			Platform::Windows,
			{1280, 720}
			});

		auto logger = &Logger();
		RendererCreateInfo rendererCreateInfo;
		rendererCreateInfo.type = RendererType::Vulkan;
		rendererCreateInfo.window = window.get();

		auto vertexShaderFileId = uuids::uuid::from_string("e1124008-e112-4008-a2f3-cf6233498020").value();
		String vertexShaderText(assetReader->GetAsset(vertexShaderFileId).Data().AsString());
		auto fragmentShaderFileId = uuids::uuid::from_string("ce637e01-1d00-405c-8aaa-f0c022235745").value();
		String fragmentShaderText(assetReader->GetAsset(fragmentShaderFileId).Data().AsString());

		VertexInputAttribute positionAttr{
			{
				{ComponentType::Float, 32},
				{ComponentType::Float, 32},
				{ComponentType::Float, 32}
			},
			0
		};

		VertexInputBinding vertexBinding{
			{
				{positionAttr}
			},
			int(sizeof(Vertex))
		};

		VertexInputLayout vertexLayout{
			{ vertexBinding	}
		};

		rendererCreateInfo.shaders = {
			{vertexShaderFileId, vertexShaderText, ShaderType::Vertex, vertexLayout},
			{fragmentShaderFileId, fragmentShaderText, ShaderType::Fragment}
		};

		logger->BeginSection("Vulkan Renderer");
		auto renderer = RendererFactory().Create(rendererCreateInfo);
		logger->EndSection();

		while (true) {
			bool close = false;
			std::optional<wc::WindowEvent> windowEvent;
			while (true) {
				windowEvent = window->GetEvent();
				if (!windowEvent.has_value()) {
					break;
				}
				else if (windowEvent.value().type == wc::WindowEvent::Type::Close) {
					close = true;
					break;
				}
			}
			if (close) {
				break;
			}

			Vertices square = MakeSquare(-0.4f, -0.4f, 0.1f);
			Vertices triangle{ {
				{-0.5f, 0.5f, 0.2f},
				{0, -0.5f, 0.2f},
				{0.5f, 0.5f, 0.2f}
			} };
			//renderer->Draw(square);
			renderer->Draw(triangle);
		}

		system("PAUSE");
	}
	catch (std::exception& e) {
		std::cout << e.what() << std::endl;
		system("PAUSE");
	}
}
#elif defined(SIS_ANDROID)
#include <android/native_activity.h>
#include "AndroidGlobals/Globals.Android.h"
// Android entry point
void ANativeActivity_onCreate(ANativeActivity* activity, void* savedState, size_t savedStateSize) {
	Sisyphus::Logger().Log("Hello from C++!");
	try {
		AndroidGlobals::InitAssetManager(activity->assetManager);

		auto assetReaderType = AssetManagement::AssetReaderType::Packed;
		auto assetReader = AssetManagement::AssetReader::Create(assetReaderType);
		assetReader->Read("assets_packed");
	}
	catch (std::exception& e) {
		Logger().Log(e.what(), LogLevel::Fatal);
	}
	catch (...) {
		Logger().Log("Unknown unhandled exception. Terminating.", LogLevel::Fatal);
	}
}
#endif