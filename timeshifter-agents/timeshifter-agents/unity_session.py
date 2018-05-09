class UnitySession:
    def __init__(self, environment, agent, brain='Brain', train_mode=True):
        self.environment = environment
        self.agent = agent
        self.brain = brain
        self.train_mode = train_mode

    def run(self):
        s, r, done = self._get_state(self.environment.reset(self.train_mode))
        a = self.agent.start(s)
        while not done:
            s, r, done = self._get_state(self.environment.step([a]))
            if not done:
                a = self.agent.step(s, r)
            else:
                self.agent.finish(r)

    def _get_state(self, unity_state):
        b = unity_state[self.brain]
        r = None if len(b.rewards) == 0 else b.rewards[0]
        return b.vector_observations[0], r, b.local_done[0]