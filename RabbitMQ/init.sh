#!/bin/bash

# Inicia o RabbitMQ em segundo plano
rabbitmq-server -detached

# Espera alguns segundos até o RabbitMQ estar pronto
rabbitmq-diagnostics await_startup

# Cria novo usuário e define permissões
rabbitmqctl add_user "Super_Admin" ".'aMN3fW[LNMJ5bp=^4,%tPCxW,9]N"
rabbitmqctl set_permissions -p / "Super_Admin" ".*" ".*" ".*"
rabbitmqctl set_user_tags "Super_Admin" administrator

# Remove o usuário guest por segurança
rabbitmqctl delete_user guest

# Traz o RabbitMQ para o primeiro plano para manter o container rodando
#rabbitmqctl shutdown
#exec rabbitmq-server