# Architecture & Deployment (summary)

- Source: public GitHub repository (this repo).
- CI: GitHub Actions runs lint/build/tests on every PR and push to main.
- CD: Deploy via Azure App Service staging slot swap (or AKS rolling update). Secrets stored in Azure Key Vault and accessed by Managed Identity. SSL/TLS terminated at Application Gateway (or Ingress with cert-manager).
- Monitoring: AppInsights (APM) + Prometheus (metrics) + ELK/Log Analytics for logs. Alerts on p95 latency and 5xx rate.
- Zero-downtime: App Service slot swap or Kubernetes rolling update (maxUnavailable: 0, readinessProbe configured).
