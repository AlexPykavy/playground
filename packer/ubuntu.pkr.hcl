variable "managed_image_resource_group_name" {
  type = string
}

variable "disk_sizes" {
  default = [127]
}

source "azure-arm" "ubuntu" {
  image_offer                       = "0001-com-ubuntu-server-jammy"
  image_publisher                   = "Canonical"
  image_sku                         = "22_04-lts"
  location                          = "East US"
  managed_image_resource_group_name = var.managed_image_resource_group_name
  os_type                           = "Linux"
  vm_size                           = "Standard_DS2_v2"
  use_azure_cli_auth                = true
}

build {
  dynamic "source" {
    for_each = var.disk_sizes
    labels   = ["azure-arm.ubuntu"]
    content {
      name = source.value

      managed_image_name = "ubuntu-22.04-${source.value}gb"
      os_disk_size_gb    = source.value
    }
  }

  provisioner "shell" {
    execute_command = "chmod +x {{ .Path }}; {{ .Vars }} sudo -E sh '{{ .Path }}'"
    inline          = ["/usr/sbin/waagent -force -deprovision+user && export HISTSIZE=0 && sync"]
    inline_shebang  = "/bin/sh -x"
  }
}