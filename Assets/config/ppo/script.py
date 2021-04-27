import yaml
import math
import os

beta = [1e-4, 1e-3, 1e-2]
gamma = [0.8, 0.9, 0.995]
hidden_units = [64,128,256]

d = dict()
agents = dict()
params = dict()
params['trainer_type'] = 'ppo'
params['keep_checkpoints'] = 5
params['checkpoint_interval'] = 500000
params['max_steps'] = 500000
params['time_horizon'] = 64
params['summary_freq'] = 50000
params['threaded'] = True
params['framework'] = 'pytorch'


hyperparams = dict()
hyperparams['batch_size'] = 1024
hyperparams['buffer_size'] = 10240
hyperparams['learning_rate'] = 0.0003
hyperparams['lambd'] = 0.95
hyperparams['num_epoch'] = 3
hyperparams['learning_rate_schedule'] = 'linear'

netparams = dict()
netparams['normalize'] = True
netparams['num_layers'] = 2
netparams['vis_encode_type'] = 'simple'

rewardparams = dict()
extrinstic = dict()
extrinstic['gamma'] = 0.99
extrinstic['strength'] = 1.0
rewardparams['extrinsic'] = extrinstic

for b in beta:
    for g in gamma:
        for h in hidden_units:
            print('Params :')
            print('beta = ' + str(b) + ' gamma = ' + str(g) + ' hidden_units = ' + str(h))
            netparams['hidden_units'] = h
            hyperparams['beta'] = b
            hyperparams['epsilon'] = g
            params['reward_signals'] = rewardparams
            params['hyperparameters'] = hyperparams
            params['network_settings'] = netparams
            agents['CarAgent'] = params
            d['behaviors'] = agents
            
            filename = 'config_' + str(b) + '_' + str(g) + '_' + str(h) + '.yaml'
            with open(filename, 'w') as outfile:
                yaml.dump(d, outfile, default_flow_style=False)
            os.system('mlagents-learn' + filename  + '--run-id=ppo_' + filename)
            os.remove(filename)


