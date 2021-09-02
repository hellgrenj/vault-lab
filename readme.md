**This is a work in progress**

## prerequisites
docker-desktop with kubernetes enabled  
kubectl installed and configured to talk to local docker-desktop cluster  
skaffold installed  

Setup Vault  
1. helm repo add hashicorp https://helm.releases.hashicorp.com  
2. helm repo update  
3. skaffold run -f skaffold.vault.yaml  
(when system is up and running its time to init and unseal Vault)  
4. kubectl exec -ti vault-0 -- vault operator init > init-output.txt  
5. repeat 3 times (3 different unseal-keys from init-output.txt) kubectl exec -ti vault-0 -- vault operator unseal 


Setup CLI
1. download and install CLI (https://www.vaultproject.io/downloads)
2. run ```export VAULT_ADDR='http://127.0.0.1:8200'```
3. run ```export VAULT_TOKEN="s.XmpNPoi9sRhYtdKHaQhkHP6x"``` (**replace token with Initial Root Token from file init-output.txt**)
4. vault login <token> (same as exported VALUE_TOKEN in previous step)

Test manually
1. in UI (localhost:8200) login with token 
2. create a new secrets engine of type kv (key value) and call it **secrets**
3. then create a secret via the CLI: ```vault kv put secrets/example-api pw=testar```


Setup Agent Injector 
1. make sure script setup-agent-injector.sh is executable  
2. run ```./setup-agent-injector.sh```   
If you cant run this bash script and/or if you want to understand how this works follow the official guide here: https://learn.hashicorp.com/tutorials/vault/agent-kubernetes?in=vault/kubernetes#configure-kubernetes-auth-method


Start API and verify that the secret gets injected to the pod

1. skaffold run -f skaffold.api.yaml

2. verify that the secret was injected:  
2.1 run ```k exec --stdin --tty api-75484987f6-2dlmk -- /bin/ash``` to shell in to the container (replace with correct pod name api-xyz)   
2.2 check that the secret exists (in json format) at /vault/secrets (```cat /vault/secrets/api```)  
3. Navigate to localhost:8080 it should now return the secret you create in step 3 under Test manually  

After restart (i.e skaffold delete -f skaffold.vault.ayml && skaffold run -f skaffold.vault.yaml)  
1. repeat step 5 from Setup Vault above
2. repeat step 2-4 from Setup CLI above
