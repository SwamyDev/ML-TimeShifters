from reinforcement.agents.td_agent import TDAgent
from reinforcement.models.q_regression_model import QRegressionModel
from reinforcement.policies.e_greedy_policies import NormalEpsilonGreedyPolicy
from reinforcement.reward_functions.q_neuronal import QNeuronal
from unityagents import UnityEnvironment
import tensorflow as tf

from unity_session import UnitySession

UNITY_BINARY = "../environment-builds/RollerBall/RollerBall.exe"
TRAIN_MODE = True
MEMORY_SIZE = 10
LEARNING_RATE = 0.01
ALPHA = 0.2
GAMMA = 0.9
N = 10

START_EPS = 1
TOTAL_EPISODES = 1000

if __name__ == '__main__':
    with UnityEnvironment(file_name=UNITY_BINARY) as env, tf.Session():
        default_brain = env.brain_names[0]
        model = QRegressionModel(4, [100], LEARNING_RATE)
        Q = QNeuronal(model, MEMORY_SIZE)
        episode = 0
        policy = NormalEpsilonGreedyPolicy(lambda: START_EPS / (episode + 1))
        agent = TDAgent(policy, Q, N, GAMMA, ALPHA)
        sess = UnitySession(env, agent, brain=default_brain, train_mode=TRAIN_MODE)

        for e in range(TOTAL_EPISODES):
            episode = e
            sess.run()
            print("Episode {} finished.".format(episode))