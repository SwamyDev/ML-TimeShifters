from collections import deque

import pytest

from unity_session import UnitySession


class BrainInfoStub:
    def __init__(self, vector_observations, rewards, done):
        self.vector_observations = vector_observations
        self.rewards = rewards
        self.local_done = done


class EnvironmentMock:
    def __init__(self, expected_train_mode=True, expected_actions=None, states=None):
        self.expected_train_mode = expected_train_mode
        self.expected_actions = None if expected_actions is None else deque(expected_actions)
        self.states = deque(states)

    def reset(self, train_mode):
        assert self.expected_train_mode == train_mode
        return self.states.popleft()

    def step(self, action):
        if self.expected_actions is not None:
            assert self.expected_actions.popleft() == action
        return self.states.popleft()


class AgentSpy:
    def __init__(self):
        self.received_start_state = None
        self.received_step_states = list()
        self.received_rewards = list()
        self.received_finish_reward = None

    def start(self, state):
        self.received_start_state = state

    def step(self, state, reward):
        self.received_step_states.append(state)
        self.received_rewards.append(reward)

    def finish(self, reward):
        self.received_finish_reward = reward


class AgentStub:
    def __init__(self, actions):
        self.actions = deque(actions)

    def start(self, state):
        return self.actions.popleft()

    def step(self, state, reward):
        return self.actions.popleft()

    def finish(self, reward):
        pass


@pytest.fixture
def agent():
    return AgentSpy()


def make_session(env, agent, brain='Brain', train=True):
    return UnitySession(env, agent, brain, train)


def make_state(state=None, rewards=None, brain='Brain', done=None):
    if done is None:
        done = [False]
    if state is None:
        state = [[]]
    if rewards is None:
        rewards = []
    return {brain: BrainInfoStub(vector_observations=state, rewards=rewards, done=done)}


def test_agent_receives_initial_state(agent):
    env = EnvironmentMock(expected_train_mode=True, states=[make_state([[1.0, 0.5]],
                                                                       brain='Brain',
                                                                       done=[True])])
    make_session(env, agent, 'Brain', True).run()
    assert agent.received_start_state == [1.0, 0.5]


def test_agent_step_receives_following_states_but_last_done_state(agent):
    env = EnvironmentMock(states=[make_state([[1.0, 0.5]]),
                                  make_state([[0.9, 0.7]]),
                                  make_state([[1.1, 0.4]]),
                                  make_state([[1.3, 0.1]], done=[True])])
    make_session(env, agent).run()
    assert agent.received_step_states == [[0.9, 0.7], [1.1, 0.4]]


def test_agent_step_receives_all_but_last_reward(agent):
    env = EnvironmentMock(states=[make_state(rewards=[]),
                                  make_state(rewards=[-0.9]),
                                  make_state(rewards=[0.7]),
                                  make_state(rewards=[0.8], done=[True])])
    make_session(env, agent).run()
    assert agent.received_rewards == [-0.9, 0.7]


def test_agent_finish_receives_last_reward(agent):
    env = EnvironmentMock(states=[make_state(rewards=[]),
                                  make_state(rewards=[-0.9]),
                                  make_state(rewards=[0.7]),
                                  make_state(rewards=[0.8], done=[True])])
    make_session(env, agent).run()
    assert agent.received_finish_reward == 0.8


def test_environment_receives_agent_action(agent):
    env = EnvironmentMock(expected_actions=[[[1]], [[0]]], states=[make_state(),
                                                                   make_state(),
                                                                   make_state(done=[True])])
    make_session(env, AgentStub(actions=[[1], [0]])).run()
