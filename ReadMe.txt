ArgoCD-Apps and ArgoCD-Tools:  Deploy with ingress


ArgoCD-Apps-MetalLB and ArgoCD-Tools-MetalLB:  Deploy with MetalLB




kubectl create secret docker-registry github-pull-secret `
  --docker-server=ghcr.io `
  --docker-username=YOUR_GITHUB_USERNAME `
  --docker-password=YOUR_PAT_TOKEN `
  -n kafka
