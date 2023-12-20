data "terraform_remote_state" "aks" {
  backend = "local"
  config = {
    path = "../aks/terraform.tfstate"
  }
}

provider "azurerm" {
  features {}
}

data "azurerm_kubernetes_cluster" "cluster" {
  name                = data.terraform_remote_state.aks.outputs.kubernetes_cluster_name
  resource_group_name = data.terraform_remote_state.aks.outputs.resource_group_name
}
provider "kubernetes" {
  alias                  = "aks"
  host                   = data.azurerm_kubernetes_cluster.cluster.kube_config.0.host
  client_certificate     = base64decode(data.azurerm_kubernetes_cluster.cluster.kube_config.0.client_certificate)
  client_key             = base64decode(data.azurerm_kubernetes_cluster.cluster.kube_config.0.client_key)
  cluster_ca_certificate = base64decode(data.azurerm_kubernetes_cluster.cluster.kube_config.0.cluster_ca_certificate)
}

resource "kubernetes_pod" "listitapi" {
  provider = kubernetes.aks

  metadata {
    name = "listitapi"
    labels = {
      "app" = "listitapi"
    }
  }

  spec {
    container {
      image = "tinasvensson/listitapi:latest"
      name  = "listitapi"

      port {
        container_port = 9001
        name           = "http"
      }
    }
  }
}

resource "kubernetes_service" "listitapi" {
  provider = kubernetes.aks
  metadata {
    name      = "listitapi"
    namespace = "default"
    labels = {
      "app" = "listitapi"
    }
  }
  spec {
    selector = {
      "app" = "listitapi"
    }
    port {
      name        = "http"
      port        = 9001
      target_port = 9001
      protocol    = "TCP"
    }
    type = "ClusterIP"
  }
}
resource "kubernetes_pod""mongo-express"{
    provider = kubernetes.aks

  metadata {
    name = "mongo-express"
    labels = {
      "app" = "mongo-express"
    }
  }

  spec {
    container {
      image = "mongo-express:latest"
      name  = "mongo-express"

      env {
        name  = "ME_CONFIG_MONGODB_SERVER"
        value = "mongodb-service"
      }
      port {
        container_port = 8081
        name           = "http"
      }      
    }     
  }  
}
resource "kubernetes_service""mongo-express"{
    provider=kubernetes.aks
    metadata{
      name="mongo-express"
      namespace="default"
      labels={
        "app"="mongo-express"
      }  
    }
    spec{
       selector = {
      "app" = "mongo-express"
    } 
    port {
      name        = "http"
      port        = 8081
      target_port = 8081
      protocol    = "TCP"
    }
    type = "LoadBalancer" 
    }
} 

# EKS resources 

data "terraform_remote_state" "eks" {
  backend = "local"
  config = {
    path = "../eks/terraform.tfstate"
  }
}

provider "aws" {
  region = data.terraform_remote_state.eks.outputs.region
}

data "aws_eks_cluster" "cluster" {
  name = data.terraform_remote_state.eks.outputs.cluster_name
}

provider "kubernetes" {
  alias                  = "eks"
  host                   = data.aws_eks_cluster.cluster.endpoint
  cluster_ca_certificate = base64decode(data.aws_eks_cluster.cluster.certificate_authority.0.data)
  exec {
    api_version = "client.authentication.k8s.io/v1beta1"
    args        = ["eks", "get-token", "--cluster-name", data.aws_eks_cluster.cluster.name]
    command     = "aws"
  }
}

resource "kubernetes_pod" "listitclient" {
  provider = kubernetes.eks

  metadata {
    name = "listitclient"
    annotations = {
      "consul.hashicorp.com/connect-service-upstreams" = "listitapi:9001:dc2"
    }
    labels = {
      "app" = "listitclient"
    }
  }

  spec {
    container {
      image = "tinasvensson/listitclient:latest"
      name  = "listitclient"

      env {
        name  = "LISTITAPI_SERVICE_URL"
        value = "http://localhost:9001"
      }

      port {
        container_port = 9002
        name           = "http"
      }
    }
  }
}

resource "kubernetes_service" "listitclient" {
  provider = kubernetes.eks

  metadata {
    name      = "listitclient-service-load-balancer"
    namespace = "default"
    labels = {
      "app" = "listitclient"
    }
  }

  spec {
    selector = {
      "app" = "listitclient"
    }
    port {
      port        = 80
      target_port = 9002
    }

    type             = "LoadBalancer"
  }
}
