# ServiceControl_exporter

[![.NET Windows Service](https://github.com/gamblen/ServiceControl_exporter/actions/workflows/ci.yml/badge.svg)](https://github.com/gamblen/ServiceControl_exporter/actions/workflows/ci.yml)

### What is it
This exporter is there to convert the values from [Particular Softwares Service Control](https://github.com/Particular/ServiceControl/) to [Prometheus](https://github.com/prometheus-net/prometheus-net).


### Metrics scraped
* Custom Checks
* Heartbeats
* Monitoring
* Unresolved Error Count

These metrics can be toggled on and off independantly of each other.