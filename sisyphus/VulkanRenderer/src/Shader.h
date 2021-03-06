#pragma once

#include "Vulkan.h"
#include "Renderer\ShaderType.h"
#include <string>

namespace Sisyphus::Rendering::Vulkan {
	class Shader {
	public:
		struct CreateInfo {
			std::string glslCode;
			ShaderType shaderType;
			vk::Device device;
		};

		Shader(const CreateInfo & ci);

		ShaderType GetType() const;
		vk::ShaderModule GetModule() const;

	private:
		ShaderType shaderType;
		vk::UniqueShaderModule shaderModule;
	};
}